import { apiClient } from "@/state/modules/apiClient";

export const actions = {
  async validateCustomer({ commit }, values) {
    return apiClient.post("Customer/Promotion/validate", values);
  },
};
