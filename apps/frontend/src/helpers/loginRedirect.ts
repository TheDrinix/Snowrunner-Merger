import type { Router } from "vue-router";

export const loginRedirect = (router: Router) => {
  const intendedRoute = localStorage.getItem("redirectAfterLogin");

  if (intendedRoute) {
    localStorage.removeItem("redirectAfterLogin");
    router.push(intendedRoute);
  }

  router.push({ name: "groups" });
};
