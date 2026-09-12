const THEME_KEY = 'theme'

export function getTheme() {
    return localStorage.getItem(THEME_KEY) || 'General'
}

export function setTheme(theme: string) {
    localStorage.setItem(THEME_KEY, theme)
}

export function clearTheme() {
    localStorage.removeItem(THEME_KEY)
}