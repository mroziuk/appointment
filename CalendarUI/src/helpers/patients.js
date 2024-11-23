import api from "./api";

export function getPatients() {
  return api.get(`/patients/all`).then(({ data: { items } }) =>
    items.map(({ id, firstName, lastName }) => ({
      id: id,
      name: `${firstName} ${lastName}`,
    }))
  );
}
export function getPatient(id) {
  return api.get(`/patients/${id}`).then((response) => response.data);
}
export function postPatient(patient) {
  return api.post("/patients", patient).then((response) => response.data);
}
export function patchPatient(id, patient) {
  return api
    .patch(`/patients/${id}`, patient)
    .then((response) => response.data);
}
export function deletePatient(id) {
  return api.delete(`/patients/${id}`).then((response) => response.data);
}
