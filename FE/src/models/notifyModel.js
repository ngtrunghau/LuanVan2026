const addMessage = (item) => {
    return {
        message: item.message,
        code: item.code
    }
}
export const notifyModel = {addMessage};
