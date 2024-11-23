import api from "./api";
export function getUsers() {
  return api.get(`/users/all`).then(({ data: { items } }) =>
    items.map(({ id, firstName, lastName }) => ({
      id: id,
      name: `${firstName} ${lastName}`,
    }))
  );
}

export function getUserMe() {
  return api.get(`/users/me`).then(({ data }) => data);
}
// export function getUser(id) {
//   return api.get(`/users/${id}`).then((response) => response.data);
// }
// export function postUser(therapist) {
//   return api.post("/users", therapist).then((response) => response.data);
// }
// export function patchUser(id, therapist) {
//   return api.patch(`/users/${id}`, therapist).then((response) => response.data);
// }
