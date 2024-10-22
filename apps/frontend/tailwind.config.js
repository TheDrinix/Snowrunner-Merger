/** @type {import('tailwindcss').Config} */
export default {
    content: [
        './index.html',
        './src/**/*.{vue,ts,tsx}'
    ],
    darkMode: 'media',
    theme: {
        extend: {},
    },
    plugins: [
        require('@tailwindcss/typography'),
        require('daisyui')
    ],
    daisyui: {
        themes: ["nord", "dracula", {
            dim: {
                ...require("daisyui/src/theming/themes")["dim"],
                primary: "rgb(72,218,230)"
            }
        }]
    }
}

