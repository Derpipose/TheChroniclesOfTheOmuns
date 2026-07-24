import { getToken } from "./tokenStorage"


export const getAuthHeader = (): Record<string, string> => { // Need to specify the return type as Record<string, string> for TypeScript
    const token = getToken()
    if (!token) {
        return {}
    }
    return { Authorization: `Bearer ${token}` }
}