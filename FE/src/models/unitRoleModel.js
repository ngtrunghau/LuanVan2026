const getJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      isDeleted : item.isDeleted,
    };
  };
  const sendJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      isDeleted : item.isDeleted,
    };
  };

  const baseJson = () => {
    return {
      name: null,
      isDeleted : false,
    };
  };

  export const unitRoleModel = {
    sendJson , getJson, baseJson,
  };
