import { FunctionComponent, useState, useEffect, useCallback } from 'react';

import UserCard from '../card/UserCardComponent';
import { User } from '../../../types';
import { deleteUserAccount } from '../../../services/userService';


type Props = {
    users: User[];
    onDeleteUser: (userId: number) => void;
};

const UserList: FunctionComponent<Props> = ({ users, onDeleteUser }) => {

    const userList = users.filter(user => user.role === 'ROLE_USER');

    const handleDeleteUser = useCallback(async (userId: number) => {
        const isDeleted = await deleteUserAccount(userId);
        if (isDeleted) {
            onDeleteUser(userId);
        }
    }, [onDeleteUser]);


    return (
        <div className="container mt-5 mb-4">
            {userList.map(user => (
                <UserCard key={user.id}
                    user={user}
                    onDelete={handleDeleteUser} />
            ))}
        </div>

    );
}

export default UserList;
