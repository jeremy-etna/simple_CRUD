import React, { FunctionComponent, useState, useEffect } from 'react'

import AddressDisplay from '../display/AddressDisplayComponent'
import AddressForm from '../form/AddressFormComponent'

import { modifyAddress, deleteAddress } from '../../../services/addressService'

import { AddressRequest, AddressResponse } from '../../../types'


type Props = {
    addressResponse: AddressResponse;
    onDelete: (userId: number) => void;
};

const AddressModeSwitcher: FunctionComponent<Props> = ({ addressResponse, onDelete }) => {
    const [isEditing, setIsEditing] = useState(false);
    const [address, setAddress] = useState<AddressResponse | null>(null);
    const [showButton, setShowButton] = useState<boolean>(true);
    const [isAdmin, setIsAdmin] = useState<boolean>(false);


    const handleEdit = (newAddress: AddressRequest) => {
        modifyAddress(addressResponse.id, newAddress)
            .then((updatedAddress) => {
                setAddress(updatedAddress);
                setIsEditing(false);
            })
            .catch(error => {
                console.error("Une erreur est survenue:", error);
            });
    };


    const handleDelete = (addressId: number) => {
        deleteAddress(addressId)
            .then(() => {
                setAddress(null);
                onDelete(addressResponse.user);
            })
            .catch(error => {
                console.error("Une erreur est survenue:", error);
            });
    };

    const handleButtonsDisplay = () => {
        setShowButton(false);

        const userStr = localStorage.getItem('user');
        if (userStr) {
            const userObj = JSON.parse(userStr);

            const userRole = userObj.role;
            if (userRole === 1) {
                setShowButton(true);
            }
            if (addressResponse.user === userObj.id) {
                console.log("User ID matches addressResponse user.");
                setShowButton(true);
            }
        } else {
            console.log("No user data in localStorage.");
        }
    }

    const checkUserRole = () => {
        const userStr = localStorage.getItem('user');
        if (userStr) {
            const userObj = JSON.parse(userStr);
            const userRole = userObj.role;
            if (userRole === 1) {
                setIsAdmin(true);
            } else {
                setIsAdmin(false);
            }
        }
    }

    useEffect(() => {
        checkUserRole();
    }, [addressResponse]);

    useEffect(() => {
        handleButtonsDisplay();
    }, [addressResponse]);

    return (
        <div className="container mt-5 p-4 border rounded">
            <hr />
            {isEditing &&
                <AddressForm addressRequest={null} addressResponse={addressResponse} onSubmit={handleEdit} isModifyMode={true} userToCreate={null} />
            }
            {!isEditing && addressResponse &&
                <AddressDisplay addressResponse={address || addressResponse} />
            }
            <div className="mt-3">
                <button className="btn btn-secondary mr-2" onClick={() => setIsEditing(!isEditing)}>
                    {isEditing ? "Cancel" : "Modify"}
                </button>
                <button className="btn btn-danger" onClick={() => handleDelete(addressResponse.id)}>Delete</button>
            </div>
            <hr />
        </div >

    );
};

export default AddressModeSwitcher
