import { useEffect, useState, FunctionComponent } from 'react';
import { User, AddressResponse } from '../../../types';
import { deleteUserAccount } from '../../../services/userService';
import { getAllAddressByuserId } from '../../../services/addressService';

import AddressModeSwitcher from '../switcher/SwitcherComponent';
import AddressForm from '../form/AddressFormComponent';


type Props = {
    user: User
    onDelete: (userId: number) => void
};


const UserCard: FunctionComponent<Props> = ({ user, onDelete }) => {
    const [showButton, setShowButton] = useState<boolean>(false);
    const [addresses, setAddresses] = useState<AddressResponse[]>([]);
    const [showAddressForm, setShowAddressForm] = useState<boolean>(false);

    useEffect(() => {
        getAllAddressByuserId(user.id).then(data => {
            setAddresses(data);
        });
    }, [user.id]);

    useEffect(() => {
        const userStr = localStorage.getItem('user');
        if (userStr) {
            const userObj = JSON.parse(userStr);
            const userRole = userObj.role;
            setShowButton(userRole === 1);
        }
    }, []);

    const handleAddAddress = () => {
        setShowAddressForm(true);
    };

    const handleDeleteAddress = (addressId: number) => {
        setAddresses(prevAddresses => prevAddresses.filter(address => address.id !== addressId));
    };

    const handleAddressAdded = (newAddress: AddressResponse) => {
        setAddresses(prevAddresses => [...prevAddresses, newAddress]);
    };


    return (
        <div className="container">
            <h1>{user.username}</h1>
            <p>{user.role}</p>
            <p>{user.creation_Date}</p>
            <p>{user.updated_Date}</p>
            {showButton && <button className="btn btn-primary" onClick={handleAddAddress}>add adress</button>}
            {showAddressForm && <AddressForm
                addressRequest={null}
                addressResponse={null}
                onSubmit={() => { }}
                isModifyMode={false}
                userToCreate={user}
                onAddressAdded={handleAddressAdded}
            />}
            <div className={'addresses'}>
                {addresses.map(address => (
                    <AddressModeSwitcher
                        key={address.id}
                        addressResponse={address}
                        onDelete={handleDeleteAddress} />
                ))}
            </div>
            {showButton && <button className="btn btn-danger" onClick={() => onDelete(user.id)}>Delete user</button>}
        </div>
    )
}

export default UserCard
