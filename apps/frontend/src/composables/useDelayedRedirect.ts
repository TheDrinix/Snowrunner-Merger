import {ref} from "vue";
import {useRouter} from "vue-router";


export function useDelayedRedirect(pageName: string, delay: number) {
    const router = useRouter();

    const del = ref(delay);
    let intervalId;

    function startRedirect() {
        intervalId = setInterval(() => {
            del.value--;

            if (del.value <= 0) {
                clearInterval(intervalId);
                router.push({ name: pageName });
            }
        }, 1000);
    }

    function stopRedirect() {
        clearInterval(pageName);
        del.value = delay
    }

    return { timer: del, startRedirect, stopRedirect }
}