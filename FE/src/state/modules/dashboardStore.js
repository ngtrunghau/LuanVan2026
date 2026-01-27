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
};
