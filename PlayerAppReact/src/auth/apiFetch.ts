import { getAuthHeader } from "./authHeader";
import { clearToken } from "./tokenStorage";

export async function apiFetch(url: string, options?: RequestInit): Promise<Response> {
  // 1. call fetch, merging getAuthHeader() into whatever headers were passed in
  const response = await fetch(url, {
    ...options,
    headers: {
      ...options?.headers,
      ...getAuthHeader(),
    },
  });

  // 2. if res.status === 401: clearToken(), redirect to /sign-in
  if (response.status === 401) {
    clearToken();
    window.location.href = '/sign-in';
    // stop further execution
    throw new Error('Timed out: Redirecting to sign-in');
  }

  // 3. return res either way
  return response;
}