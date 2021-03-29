import React, { useState } from 'react';
import { Resource } from '../translations/resource';
import { resources } from './resources';
import { TenantList } from './TenantList';
import { TenantForm } from './TenantForm';
import { useTenants } from './hooks';
import './styles.css';

export const Tenants = () => {
    const { tenants } = useTenants();
    const [selectedTenant, setSelectedTenant] = useState(null);
    const onSelectTenant = (id) => setSelectedTenant(id);

    return (
        <>
            <h1 className="header">
                <Resource id={"header"} resources={resources}>Manage Tenants</Resource>
            </h1>
            {selectedTenant === null ? (
                <TenantList tenants={tenants} onSelectTenant={onSelectTenant} />
            ) : (
                <TenantForm id={selectedTenant} removeSelectedTenant={() => onSelectTenant(null)}  />
            )}
        </>
    );
}
