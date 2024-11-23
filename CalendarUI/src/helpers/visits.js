import api from "./api";
export const getVisits = async ({ selected }) =>
  await api.get("/visits/all").then(({ data: { items } }) =>
    items.map(({ id, startDate, endDate, roomId, therapistId, patientId }) => ({
      id,
      start: startDate,
      end: endDate,
      roomId: roomId,
      therapistId: therapistId,
      patientId: patientId,
      resource: selected === "room" ? roomId : therapistId,
    }))
  );
export async function getVisit(id) {
  return await api.get(`/visits/${id}`).then((response) => response.data);
}
export function postVisit(visit) {
  return api.post("/visits", visit).then((response) => response.data);
}
export async function patchVisit({ id, visit }) {
  const response = await api.patch(`/visits/${id}`, visit);
  return response.data;
}
export function deleteVisit(id) {
  api.delete(`/visits/${id}`).then((response) => response.data);
}
