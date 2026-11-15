import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getPlayer, setPlayerTheme } from "../api/playerApi";
import { useAuth } from "../auth/AuthContext";
import { setTheme } from "../theme/themeStorage";

export function usePlayer() {
    const { isAuthenticated } = useAuth()
    return useQuery({
        queryKey: ['player'],
        queryFn: getPlayer,
        enabled: isAuthenticated, // Only fetch player data if a token exists
    })
}

export function useSetPlayerTheme() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (theme: string) => setPlayerTheme(theme),
        onSuccess: (_data, theme) => {
            setTheme(theme)
            document.documentElement.setAttribute('data-theme', theme)
            queryClient.invalidateQueries({ queryKey: ['player'] })
        },
    })
}
