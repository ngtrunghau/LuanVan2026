import { apiClient } from "@/state/modules/apiClient";

export const actions = {
  async getAlerts({ commit }, params = {}) {
    const query = new URLSearchParams();
    query.append("includeAll", String(Boolean(params.includeAll)));
    query.append("defaultThreshold", String(params.defaultThreshold ?? 5));
    return apiClient.get(`Analytics/stock-alerts?${query.toString()}`);
  },
  async setThreshold({ commit }, values) {
    return apiClient.post("Analytics/stock-threshold", values);
  },
};
