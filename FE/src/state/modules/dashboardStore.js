import {apiClient} from "@/state/modules/apiClient";
const controller = "Chart";

export const state = {
    reloadAuthUser: false,
}

export const mutations = {
    SET_RELOADAUTHUSER(state, newValue) {
        state.reloadAuthUser = newValue
    }
}
export const actions = {
    async getAll({commit}) {
        return apiClient.get(controller + "/get-doanh-thu-thang");
    },
    async getRevenueOverview({commit}, params = {}) {
        const query = new URLSearchParams();
        if (params.fromDate) query.append("fromDate", params.fromDate);
        if (params.toDate) query.append("toDate", params.toDate);
        if (params.compareFromDate) query.append("compareFromDate", params.compareFromDate);
        if (params.compareToDate) query.append("compareToDate", params.compareToDate);
        return apiClient.get("Analytics/revenue-overview" + (query.toString() ? `?${query.toString()}` : ""));
    },
    async getRevenueTrend({commit}, params = {}) {
        const query = new URLSearchParams();
        if (params.groupBy) query.append("groupBy", params.groupBy);
        if (params.fromDate) query.append("fromDate", params.fromDate);
        if (params.toDate) query.append("toDate", params.toDate);
        return apiClient.get("Analytics/revenue-trend" + (query.toString() ? `?${query.toString()}` : ""));
    },
    async getRevenueByCategory({commit}, params = {}) {
        const query = new URLSearchParams();
        if (params.fromDate) query.append("fromDate", params.fromDate);
        if (params.toDate) query.append("toDate", params.toDate);
        return apiClient.get("Analytics/revenue-by-category" + (query.toString() ? `?${query.toString()}` : ""));
    },
    async getRevenueByPaymentMethod({commit}, params = {}) {
        const query = new URLSearchParams();
        if (params.fromDate) query.append("fromDate", params.fromDate);
        if (params.toDate) query.append("toDate", params.toDate);
        return apiClient.get("Analytics/revenue-by-payment-method" + (query.toString() ? `?${query.toString()}` : ""));
    },
    async getInventoryInsights({commit}, params = {}) {
        const query = new URLSearchParams();
        if (params.lookbackDays) query.append("lookbackDays", params.lookbackDays);
        if (params.forecastDays) query.append("forecastDays", params.forecastDays);
        return apiClient.get("Analytics/inventory-insights" + (query.toString() ? `?${query.toString()}` : ""));
    },
};
