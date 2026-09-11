import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getPlayer, setPlayerTheme } from "../api/playerApi";
import { getToken } from "../auth/tokenStorage";
import { setTheme } from "../theme/themeStorage";

export function usePlayer() {
    return useQuery({
        queryKey: ['player'],
        queryFn: getPlayer,
        enabled: !!getToken(), // Only fetch player data if a token exists
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
