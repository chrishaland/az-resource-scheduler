import React, { useState } from 'react';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { EnvironmentList } from './EnvironmentList';
import { EnvironmentForm } from './EnvironmentForm';
import { useEnvironments } from './hooks';
import './styles.css';

export const Environments = () => {
    const { environments, refetchEnvironments } = useEnvironments();
    const [selectedEnvironment, setSelectedEnvironment] = useState(null);
    const onSelectEnvironment = (id) => setSelectedEnvironment(id);
    
    const removeSelectedEnvironment = () => { 
        onSelectEnvironment(null);
        refetchEnvironments(); 
    };
    return (
        <>
            <h1 className="header">
                <Locale id={"header"} locales={locales}>Manage Environments</Locale>
            </h1>
            {selectedEnvironment === null ? (
                <EnvironmentList environments={environments} onSelectEnvironment={onSelectEnvironment} />
            ) : (
                <EnvironmentForm id={selectedEnvironment} onSelectEnvironment={onSelectEnvironment} removeSelectedEnvironment={removeSelectedEnvironment} />
            )}
        </>
    );
}
