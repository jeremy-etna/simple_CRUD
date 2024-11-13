import { FunctionComponent, useState, useEffect } from 'react'

import AddressForm from '../components/profile/form/AddressFormComponent'
import AddressList from '../components/profile/list/AddressListComponent'
import DeleteUserButton from '../components/profile/delete/DeleteButtonComponent'

import { getAllAddressByuserId } from '../services/addressService';
import { AddressRequest, AddressResponse } from '../types'


const Profile: FunctionComponent = () => {
    const [address, setAddress] = useState<AddressRequest | null>(null);
    const [allAddress, setAllAddress] = useState<AddressResponse[]>([]);

    const loadData = () => {
        const userStr = localStorage.getItem('user');
        const userId = userStr ? JSON.parse(userStr).id : null;

        getAllAddressByuserId(userId)
            .then(data => setAllAddress(data))
            .catch(error => console.error('Erreur lors de la récupération des adresses:', error));
    };

    useEffect(() => {
        loadData();
    }, []);

    const handleAddressSubmit = (address: AddressRequest) => {
        setAddress(address);
        loadData();
    };

    const handleDelete = () => {
        loadData();
    };



    return (
        <main className="container">
            <h1>Add new address</h1>
            <AddressForm addressRequest={address} addressResponse={null} onSubmit={handleAddressSubmit} isModifyMode={false} userToCreate={null} />
            <h1>Address list</h1>
            <AddressList allAddress={allAddress} onDelete={handleDelete} />
            <DeleteUserButton />
        </main>
    )
}

export default Profile