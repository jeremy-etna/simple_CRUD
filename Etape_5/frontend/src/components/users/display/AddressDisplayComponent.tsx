import { FunctionComponent, useEffect, useState } from 'react'

import { AddressResponse } from '../../../types'


type Props = {
    addressResponse: AddressResponse
}

const renderField = (label: string, value: string) => (
    <div className="mb-2">
        <span className="text-uppercase font-weight-bold">{label}:</span> {value}
    </div>
)

const AddressDisplay: FunctionComponent<Props> = ({ addressResponse }) => {
    const [address, setAddress] = useState<AddressResponse | null>(addressResponse)

    useEffect(() => {
        setAddress(addressResponse);
    }, [addressResponse]);

    return (
        <div className="container mt-5 p-4 shadow bg-light rounded">
            {renderField("Street", address?.street || '')}
            {renderField("Postal code", address?.postalCode || '')}
            {renderField("City", address?.city || '')}
            {renderField("Country", address?.country || '')}
        </div>
    )
}

export default AddressDisplay
