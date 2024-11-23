import api from "./api";

export function getRooms() {
  return api.get(`/rooms/all`).then((res) => res.data.items);
}

export function getRoom(id) {
  return api.get(`/rooms/${id}`).then((response) => response.data);
}

export function postRoom(room) {
  return api.post("/rooms", room).then((response) => response.data);
}

export function patchRoom(id, room) {
  return api.patch(`/rooms/${id}`, room).then((response) => response.data);
}
