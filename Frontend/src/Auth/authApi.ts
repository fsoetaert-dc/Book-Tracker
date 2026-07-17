import { apiRequest } from "../api";
import type { CurrentMember, LoginRequest, LoginResponse } from "./Types";

export function login(request: LoginRequest) {
  return apiRequest<LoginResponse>("/auth/login", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getCurrentMember() {
  return apiRequest<CurrentMember>("/auth/me");
}