import api from "./api";
export function getAvailability() {
  return api.get(`/availability/all`).then((response) => response.data);
}

export function getAvailabilities(id) {
  return api.get(`/availability/${id}`).then((response) => response.data);
}
export function postAvailability(availability) {
  return api
    .post("/availability", availability)
    .then((response) => response.data);
}
export function patchAvailability(id, availability) {
  return api
    .patch(`/availability/${id}`, availability)
    .then((response) => response.data);
}
export function deleteAvailability(id) {
  return api.delete(`/availability/${id}`).then((response) => response.data);
}
