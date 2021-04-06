import React, { useState } from 'react';
import { Resource } from '../translations/resource';
import { resources } from './resources';
import { EnvironmentList } from './EnvironmentList';
import { EnvironmentForm } from './EnvironmentForm';
import { useEnvironments } from './hooks';
import './styles.css';

export const Environments = () => {
    const { environments } = useEnvironments();
    const [selectedEnvironment, setSelectedEnvironment] = useState(null);
    const onSelectEnvironment = (id) => setSelectedEnvironment(id);

    return (
        <>
            <h1 className="header">
                <Resource id={"header"} resources={resources}>Manage Environments</Resource>
            </h1>
            {selectedEnvironment === null ? (
                <EnvironmentList environments={environments} onSelectEnvironment={onSelectEnvironment} />
            ) : (
                <EnvironmentForm id={selectedEnvironment} onSelectEnvironment={onSelectEnvironment} removeSelectedEnvironment={() => onSelectEnvironment(null)}  />
            )}
        </>
    );
}
