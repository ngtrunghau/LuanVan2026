import { apiClient } from "@/state/modules/apiClient";

const controller = "Customer/Assistant";

export const actions = {
    async ask(context, payload) {
        return apiClient.post(controller + "/ask", payload);
    },
};
