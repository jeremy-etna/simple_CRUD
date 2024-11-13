import { FunctionComponent, useState, useEffect } from 'react';

import AddressModeSwitcher from '../switcher/SwitcherComponent';
import { AddressResponse } from '../../../types';


type Props = {
    allAddress: AddressResponse[];
    onDelete: () => void;
};

const AddressList: FunctionComponent<Props> = ({ allAddress, onDelete }) => {
    return (
        <div className="container">
            {allAddress.map(address => (
                <AddressModeSwitcher key={address.id} addressResponse={address} onDelete={onDelete} />
            ))}
        </div>
    );
}

export default AddressList;