import { useMutation } from "@tanstack/react-query";
import { authApi } from "@/features/auth";
import { useNavigate } from "react-router";

export const useLogout = () => {
  const navigate = useNavigate();

  return useMutation({
    mutationKey: ["auth", "logout"],
    mutationFn: authApi.logout,
    onSuccess: () => {
      navigate("/", { replace: true });
    },
    retry: false,
  })
}