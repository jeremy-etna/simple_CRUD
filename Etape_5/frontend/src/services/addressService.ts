import { AddressRequest, AddressResponse, User } from '../types'


const apiUrl = process.env.REACT_APP_API_URL


export const createAddress = async (addressRequest: AddressRequest, user: User | null): Promise<AddressResponse> => {
    const token = localStorage.getItem('token')

    try {
        let response: Response;

        if (user !== null) {
            const userId = user.id;
            response = await fetch(`${apiUrl}/user/${userId}/address`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(addressRequest)
            });
        } else {
            response = await fetch(`${apiUrl}/address`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(addressRequest)
            });
        }

        if (!response.ok) {
            throw new Error('Failed to post the address');
        }

        // Convert the response to JSON and return it.
        return response.json();

    } catch (error) {
        console.error('An error occurred:', error);
        throw error; // This will propagate the error to the caller.
    }
};




export const modifyAddress = async (addressId: number, newAddress: AddressRequest): Promise<AddressResponse> => {
    const token = localStorage.getItem('token')
    try {
        const response = await fetch(`${apiUrl}/address/${addressId}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newAddress)
        });

        if (!response.ok) {
            throw new Error('Failed to modify the address');
        }

        const updatedAddress: AddressResponse = await response.json();
        return updatedAddress;

    } catch (error) {
        console.error('An error occurred:', error);
        throw error;
    }
};


export const deleteAddress = async (addressId: number): Promise<void> => {
    const token = localStorage.getItem('token')
    try {
        const response = await fetch(`${apiUrl}/address/${addressId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            throw new Error('Failed to modifiy the address');
        }
    } catch (error) {
        console.error('An error occurred:', error);
    }
};


export const getAllAddress = async (): Promise<AddressResponse[]> => {
    const token = localStorage.getItem('token')
    try {
        const response = await fetch(`${apiUrl}/address`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('Failed to fetch all the address');
        }
        return await response.json();
    } catch (error) {
        console.error('An error occurred:', error);
        return [];
    }
};

export const getAllAddressByuserId = async (userId: number): Promise<AddressResponse[]> => {
    const token = localStorage.getItem('token')
    try {
        const response = await fetch(`${apiUrl}/user/${userId}/addresses`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error('Failed to fetch all the address');
        }
        return await response.json();
    } catch (error) {
        console.error('An error occurred:', error);
        return [];
    }
};