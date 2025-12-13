import Cookies from 'js-cookie'

const TokenKey = 'Admin-Token'

export function getToken() {
  return localStorage.getItem("token");
  //return Cookies.get(TokenKey)
}

export function setToken(token) {
  localStorage.setItem("token", token);
  return Cookies.set(TokenKey, token)
}

export function removeToken() {
  localStorage.removeItem("token");
  //return Cookies.remove(TokenKey)
}
