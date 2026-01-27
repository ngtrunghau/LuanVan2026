const getJson = (item) => {
    return {
        id: item.id,
        userName: item.userName,
        name: item.name,
        unitRole: item.unitRole,
        unitRoleId: item.unitRoleId,
        isDeleted: item.isDeleted,
        password: item.password
    }
}

const sendJson = (item) => {
    return {
        id: item.id,
        userName: item.userName,
        name: item.name,
        unitRole: item.unitRole,
        unitRoleId: item.unitRoleId,
        isDeleted: item.isDeleted,
        password: item.password
    }
}

const baseJson = () => {
    return {
        userName: null,
        name: null,
        unitRoleId: null,
        unitRole: null,
        isDeleted: false,
        password: null
    }
}

export const userModel = {
    sendJson, getJson, baseJson
}
