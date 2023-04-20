

export class TypesModel {
    id: number;
    name: string;
    description: string;
    financeProductType: string;
    financeProductTypeCode: number;
    isActive: boolean;
}

export class CategoriesModel {
    id: number;
    typeID: number;
    name: string;
    description: string;
    financeProductType: string;
    financeProductTypeCode: number;
    isActive: boolean;
}

export class StatusModel {
    id: number;
    description: string;
}

