export interface User {
    id: number,
    username: string,
    password: string,
    role: string,
    creation_Date: string,
    updated_Date: string
}

export interface AddressRequest {
    street: string,
    postalCode: string,
    city: string,
    country: string,
}

export interface AddressResponse {
    id: number,
    street: string,
    postalCode: string,
    city: string,
    country: string,
    user: number,
    creationDate: string,
    updatedDate: string
}

export interface CurrentUser {
    id: number,
    username: string,
    role: string,
}

export interface RegisterFormData {
    role: string;
    username: string;
    password: string;
}