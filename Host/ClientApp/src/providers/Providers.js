import React, { useState } from 'react';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { ProviderList } from './ProviderList';
import { ProviderForm } from './ProviderForm';
import { useProviders } from './hooks';
import './styles.css';

export const Providers = () => {
    const { providers, refetchProviders } = useProviders();
    const [selectedProvider, setSelectedProvider] = useState(null);
    
    const onSelectProvider = (id) => setSelectedProvider(id);
    
    const removeSelectedProvider = () => { 
        onSelectProvider(null);
        refetchProviders(); 
    };

    return (
        <>
            <h1 className="header">
                <Locale id={"header"} locales={locales}>Manage Cloud Providers</Locale>
            </h1>
            {selectedProvider === null ? (
                <ProviderList providers={providers} onSelectProvider={onSelectProvider} />
            ) : (
                <ProviderForm id={selectedProvider} onSelectProvider={onSelectProvider} removeSelectedProvider={removeSelectedProvider}  />
            )}
        </>
    );
}
