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
}

