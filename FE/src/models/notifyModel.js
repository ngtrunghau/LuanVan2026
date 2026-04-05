const addMessage = (item) => {
    const safeItem = item || {};
    return {
        message: safeItem.message || "Có lỗi xảy ra",
        code: safeItem.code ?? -1
    };
}
export const notifyModel = {addMessage};
