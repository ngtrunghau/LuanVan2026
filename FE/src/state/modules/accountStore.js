import { apiClient } from "@/state/modules/apiClient";

export const actions = {
  async changePassword({ commit }, values) {
    return apiClient.post("Account/change-password", values);
  },
};
