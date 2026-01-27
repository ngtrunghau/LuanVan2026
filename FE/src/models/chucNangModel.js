const getJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      router: item.router,
      controllerId: item.controllerId,
      unitRoleId: item.unitRoleId,
      key: item.key,
      isDeleted : item.isDeleted,
    };
  };
  const sendJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      router: item.router,
      controllerId: item.controllerId,
      unitRoleId: item.unitRoleId,
      key: item.key,
      isDeleted : item.isDeleted,
    };
  };

  const baseJson = () => {
    return {
      name: null,
      router: null,
      controllerId: null,
      unitRoleId: null,
      key: "01",
      isDeleted : false,
    };
  };

  export const chucNangModel = {
    sendJson , getJson, baseJson,
  };
