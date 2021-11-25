import React, { useState } from 'react';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { ResourceList } from './ResourceList';
import { ResourceForm } from './ResourceForm';
import { useResources } from './hooks';
import './styles.css';

export const Resources = () => {
    const { resources, refetchResources } = useResources();
    const [selectedResource, setSelectedResource] = useState(null);
    
    const onSelectResource = (id) => setSelectedResource(id);
    
    const removeSelectedResource = () => { 
        onSelectResource(null);
        refetchResources(); 
    };

    return (
        <>
            <h1 className="header">
                <Locale id={"header"} locales={locales}>Manage Resources</Locale>
            </h1>
            {selectedResource === null ? (
                <ResourceList resources={resources} onSelectResource={onSelectResource} />
            ) : (
                <ResourceForm id={selectedResource} onSelectResource={onSelectResource} removeSelectedResource={removeSelectedResource}  />
            )}
        </>
    );
}
