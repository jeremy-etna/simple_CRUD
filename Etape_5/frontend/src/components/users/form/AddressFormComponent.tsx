import { useState, FunctionComponent } from 'react'

import { User } from '../../../types'
import { createAddress, modifyAddress } from '../../../services/addressService'
import { AddressRequest, AddressResponse } from '../../../types'


type Props = {
    addressRequest: AddressRequest | null;
    addressResponse: AddressResponse | null;
    onSubmit: (addressRequest: AddressRequest) => void;
    isModifyMode: boolean;
    userToCreate: User | null;
    onAddressAdded?: (address: AddressResponse) => void;
}

const AddressForm: FunctionComponent<Props> = ({ addressRequest, addressResponse, onSubmit, isModifyMode, userToCreate, onAddressAdded }) => {
    const [street, setStreet] = useState(addressRequest ? addressRequest.street : '');
    const [postalCode, setPostalCode] = useState(addressRequest ? addressRequest.postalCode : '');
    const [city, setCity] = useState(addressRequest ? addressRequest.city : '');
    const [country, setCountry] = useState(addressRequest ? addressRequest.country : '');

    const renderInputField = (label: string, value: string, onChange: (value: string) => void) => (
        <div className="form-group mb-3">
            <label>{label}:</label>
            <input type="text" value={value} onChange={e => onChange(e.target.value)} className="form-control" />
        </div>

    )

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        const address: AddressRequest = { street, postalCode, city, country };

        if (isModifyMode === true && addressResponse !== null) {
            modifyAddress(addressResponse.id, address)
                .then(() => {
                    onSubmit(address);
                })
                .catch(error => {
                    console.error("Une erreur est survenue:", error);
                });
        }

        if (!isModifyMode) {
            const user = userToCreate;
            createAddress(address, user)
                .then((newAddress: AddressResponse) => {
                    onSubmit(address);
                    if (onAddressAdded) onAddressAdded(newAddress);
                })
                .catch(error => {
                    console.error("Une erreur est survenue:", error);
                });
        }

    };

    const handleEditMode = (address: AddressResponse) => {
        setStreet(address.street);
        setPostalCode(address.postalCode);
        setCity(address.city);
        setCountry(address.country);
    };

    return (
        <form onSubmit={handleSubmit} className="mt-4">
            {renderInputField("Street", street, setStreet)}
            {renderInputField("Postal code", postalCode, setPostalCode)}
            {renderInputField("City", city, setCity)}
            {renderInputField("Country", country, setCountry)}
            <button type="submit" className="btn btn-primary">Save</button>
        </form>

    )
}

export default AddressForm
