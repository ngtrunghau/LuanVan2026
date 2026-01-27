const getJson = (item) => {
    return {
        id: item.id,
        productId: item.productId,
        product: item.product,
        name: item.name,
        quantityImport: item.quantityImport,
        isDeleted: item.isDeleted,
        remainQuantity: item.remainQuantity
    }
}

const sendJson = (item) => {
    return {
        id: item.id,
        productId: item.productId,
        name: item.name,
        quantityImport: item.quantityImport,
        product: item.product,
        isDeleted: item.isDeleted,
        remainQuantity: item.remainQuantity
    }
}

const baseJson = () => {
    return {
        productId: null,
        product: null,
        name: null,
        quantityImport: 0,
        isDeleted: false,
        remainQuantity: 0
    }
}

export const khoModel = {
    sendJson, getJson, baseJson
}
