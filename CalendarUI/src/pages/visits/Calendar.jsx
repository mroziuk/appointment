import { useState, useEffect, useRef } from "react";
import {
  DayPilot,
  DayPilotCalendar,
  DayPilotNavigator,
} from "@daypilot/daypilot-lite-react";
import "./Calendar.css";
import ResourceGroups from "./ResourceGroups.jsx";
import { ErrorBoundary } from "react-error-boundary";
import { Toaster, toast } from "sonner";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getRooms } from "../../helpers/rooms.js";
import { getUsers } from "../../helpers/users.js";
import { getPatients } from "../../helpers/patients.js";
import {
  getVisits,
  getVisit,
  patchVisit,
  deleteVisit,
  postVisit,
} from "../../helpers/visits.js";

const Calendar = () => {
  const calendarRef = useRef();
  const queryClient = useQueryClient();
  const { mutate: editVisit } = useMutation({
    mutationFn: patchVisit,
    onSuccess: () => {
      toast.success("Visit updated successfully");
      queryClient.invalidateQueries("visits", selected.id, startDate);
    },
    onError: (error) => {
      toast.error(error.response.data.Message);
      queryClient.invalidateQueries("visits", selected.id, startDate);
    },
  });
  const { mutate: delVisit } = useMutation({
    mutationFn: deleteVisit,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["visits", selected.id, startDate],
      });
      toast.success("Visit deleted successfully");
    },
    onError: (error) => {
      toast.error(error.response.data.Message);

    },
  });
  const { mutate: addVisit } = useMutation({
    mutationFn: postVisit,
    onSuccess: () => {
      queryClient.invalidateQueries("visits", selected.id, startDate);
      toast.success("Visit added successfully");
    },
    onError: (error) => {
      toast.error(error.response.data.Message);
    },
  });
  const [selected, setSelected] = useState({ id: "room", resources: [] });
  const [groups, setGroups] = useState([]);
  const [columns, setColumns] = useState([]);
  const [startDate, setStartDate] = useState(new DayPilot.Date("2024-03-10"));

  const { data: rooms } = useQuery({
    queryKey: ["rooms"],
    queryFn: getRooms,
  });
  const { data: therapists } = useQuery({
    queryKey: ["therapists"],
    queryFn: getUsers,
  });
  const { data: patients } = useQuery({
    queryKey: ["patients"],
    queryFn: getPatients,
  });
  const { data: visits } = useQuery({
    queryKey: ["visits", selected.id, startDate],
    queryFn: () =>
      getVisits({
        selected: selected.id,
        patients: patients,
        rooms: rooms,
        therapists: therapists,
      }),
    enabled:
      rooms &&
      rooms.length > 0 &&
      therapists &&
      therapists.length > 0 &&
      patients &&
      patients.length > 0,
  });

  //const error = errorRoom || errorTherapist || errorPatient || errorVisit;
  //const isLoading =  isLoadingRoom || isLoadingTherapist || isLoadingPatient || isLoadingVisit;
  // if (error) {
  //   throw new Error(error);
  // }
  useEffect(() => {
    const data = [
      { name: "Room", id: "room", resources: rooms },
      { name: "Therapist", id: "therapist", resources: therapists },
    ];
    setGroups(data);  
    setSelected(data[0]);
  }, [rooms, therapists]);
  useEffect(() => {
    setColumns(selected?.resources || []);
  }, [selected, groups]);
  useEffect(() => {
    setConfig({
      viewType: "Resources",
    });
  }, []);

  const onEventClick = async (args) => {
    const form = [
      {
        name: "Start",
        id: "start",
        dateFormat: "dd.MM.yyyy",
        timeFormat: "H:mm",
        type: "datetime",
      },
      {
        name: "End",
        id: "end",
        dateFormat: "dd.MM.yyyy",
        timeFormat: "H:mm",
        type: "datetime",
      },
      {
        name: "Therapist",
        id: "therapist",
        options: therapists,
        type: "serchable",
      },
      {
        name: "Room",
        id: "room",
        options: rooms,
      },
      {
        type: "searchable",
        name: "Patient",
        id: "patient",
        options: patients,
      },
    ];
    const visit = await getVisit(args.e.data.id);
    const data = {
      start: DayPilot.Date(visit.startDate),
      end: DayPilot.Date(visit.endDate),
      room: visit.roomId,
      patient: visit.patientId,
      therapist: visit.therapistId,
    };
    const options = {
      focus: "therapist",
    };
    const modal = await DayPilot.Modal.form(form, data, options);
    if (!modal.result) {
      return;
    }
    editVisit({
      id: args.e.data.id,
      visit: {
        DateStart: modal.result.start.value,
        DateEnd: modal.result.end.value,
        RoomId: modal.result.room,
        PatientId: modal.result.patient,
        TherapistId: modal.result.therapist,
      },
    });
  };
  const onEventDelete = async (args) => {
    args.preventDefault();
    const modal = await DayPilot.Modal.confirm(
      "Do you really want to delete the event?"
    );
    if (modal.canceled) {
      return;
    }
    delVisit(args.e.data.id);
  };
  const onTimeRangeSelected = async (args) => {
    const form = [
      {
        name: "Start",
        id: "start",
        dateFormat: "dd.MM.yyyy",
        timeFormat: "H:mm",
        type: "datetime",
      },
      {
        name: "End",
        id: "end",
        dateFormat: "dd.MM.yyyy",
        timeFormat: "H:mm",
        type: "datetime",
      },
      {
        name: "Room",
        id: "room",
        options: rooms,
      },
      {
        name: "Therapist",
        id: "therapist",
        options: therapists,
      },
      {
        name: "Patient",
        id: "patient",
        options: patients,
      },
    ];
    const data = {
      start: args.start,
      end: args.end,
      room: selected.id === "room" ? selected.resources[0].id : rooms[0].id,
      therapist: selected.id === "therapist" ? selected.resources[0].id : null,
    };
    const modal = await DayPilot.Modal.form(form, data, { focus: "patient" });
    if (!modal.result) {
      return;
    }
    const visit = {
      DateStart: modal.result.start,
      DateEnd: modal.result.end,
      TherapistId: modal.result.therapist,
      PatientId: modal.result.patient,
      RoomId: modal.result.room,
      IsRecuring: false,
    };
    addVisit(visit);
  };
  const onEventResized = async (args) => {
    editVisit({
      id: args.e.data.id,
      visit: {
        DateStart: args.newStart,
        DateEnd: args.newEnd,
      },
    });
  };
  const resouceColors = ["#baf2bb", "#baf2d8", "#bad7f2", "#f2bac9", "#f2e2ba"];
  const getColor = (id) => {
    return resouceColors[id % resouceColors.length];
  };
  const onBeforeEventRender = ({ data }) => {
    data.text =
      patients.find(({ id }) => id === data.patientId).name +
      " - " +
      (selected === "room"
        ? rooms.find(({ id }) => id === data.roomId).name
        : therapists.find(({ id }) => id === data.therapistId).name);

    data.backColor = getColor(data.therapistId);
  };
  const onEventMoved = async (args) => {
    args.preventDefault();
    const data = {
      DateStart: args.newStart,
      DateEnd: args.newEnd,
    };
    selected.id === "room" && (data.RoomId = args.newResource);
    selected.id === "therapist" && (data.TherapistId = args.newResource);
    editVisit({ id: args.e.data.id, visit: data });
  };
  const [config, setConfig] = useState({
    viewType: "Resources",
    timeRangeSelectedHandling: "Enabled",
    eventDeleteHandling: "Update",
  });
  const next = () => {
    setStartDate(startDate.addDays(1));
  };
  const previous = () => {
    setStartDate(startDate.addDays(-1));
  };
  // State for startDate
  return (
    <ErrorBoundary>
      <Toaster richColors />
      <div className="wrap">
        <div className={"left"}>
          <DayPilotNavigator
            selectMode={"Day"}
            showMonths={3}
            skipMonths={3}
            // selectionDay={startDate}
            startDate={startDate}
            onTimeRangeSelected={(args) => setStartDate(args.day)}
          />
        </div>
        <div className={"calendar"}>
          <div className={"toolbar"}>
            <ResourceGroups
              onChange={(args) => setSelected(args.selected)}
              items={groups}
            ></ResourceGroups>
            <span>Day:</span>
            <button onClick={() => previous()}>Previous</button>
            <button onClick={() => next()}>Next</button>
          </div>
          <DayPilotCalendar
            {...config}
            columns={columns}
            events={visits}
            startDate={startDate}
            onEventClick={onEventClick}
            onEventDelete={onEventDelete}
            onTimeRangeSelected={onTimeRangeSelected}
            onEventMoved={onEventMoved}
            onEventResized={onEventResized}
            onBeforeEventRender={onBeforeEventRender}
            ref={calendarRef}
          />
        </div>
      </div>
    </ErrorBoundary>
  );
};

export default Calendar;
