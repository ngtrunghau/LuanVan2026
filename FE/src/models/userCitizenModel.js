const getJson = (item) => {
    return {
        id: item.id,
        userName: item.userName,
        fullName: item.fullName,
        phoneNumber: item.phoneNumber,
        email: item.email,
        note: item.note,
        avatar: item.avatar,
        unitRole: item.unitRole,
        permissions: item.permissions,
        menu: item.menu,
        soCCCD: item.soCCCD,
        name: item.name,
        tokenFCM : item.tokenFCM
    }
}

const sendJson = (item) => {
    return {
        id: item.id,
        userName: item.userName,
        firstName: item.firstName,
        lastName: item.lastName,
        fullName: item.fullName,
        phoneNumber: item.phoneNumber,
        email: item.email,
        note: item.note,
        avatar: item.avatar,
        unitRole: item.unitRole,
        permissions: item.permissions,
        menu: item.menu,
        soCCCD: item.soCCCD,
        name: item.name,
        tokenFCM : item.tokenFCM
    }
}

const baseJson = () => {
    return {
        id: null,
        userName: null,
        fullName: null,
        phoneNumber: null,
        email: null,
        note: null,
        avatar: null,
        unitRole: null,
        permissions: null,
        menu: null,
        soCCCD: null,
        name: null,
        tokenFCM : null

    }
}

export const userCitizenModel = {
    sendJson, getJson, baseJson
}
