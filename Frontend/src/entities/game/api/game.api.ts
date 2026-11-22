import type { PageParams } from "@/shared/api/pagination.ts";
import type { BackUrlResponse, GameDto, GameInviteInfo } from "@/entities/game";
import { http } from "@/shared/api/axios.ts";

export const gameApi = {
  getAll: async ({ skip = 0, take = 20 } : PageParams) => {
    const { data } = await http.get<GameDto[]>(`/games?skip=${skip}&take=${take}`)
    return data;
  },
  getById: async (id: string) => {
    const { data } = await http.get<GameDto>(`/games/${id}`)
    return data;
  },
  getByCode: async (code: string) => {
    const { data } = await http.get<BackUrlResponse>(`/invites/join-game/by-code?code=${code}`);
    return data.backUrl;
  },
  getInviteInfo: async (gameId: string) => {
    const { data } = await http.get<GameInviteInfo>(`/runtime/games/${gameId}`);
    return data;
  }
}