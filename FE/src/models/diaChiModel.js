const getJson = (item) => {
    return {
      id: item.id,
      address: item.address,
      provinceId : item.provinceId,
      districtId : item.districtId,
      townId : item.townId,
      customerId : item.customerId,
      isDeleted: item.isDeleted,
    };
  };
  const sendJson = (item) => {
    return {
      id: item.id,
      address: item.address,
      provinceId : item.provinceId,
      districtId : item.districtId,
      townId : item.townId,
      customerId : item.customerId,
      isDeleted: item.isDeleted,
    };
  };

  const baseJson = () => {
    return {
      address: null,
      provinceId : null,
      districtId : null,
      townId : null,
      customerId : null,
      isDeleted: false,
    };
  };

  export const diaChiModel = {
    sendJson , getJson, baseJson,
  };
