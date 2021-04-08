import { useState, useEffect } from "react";
import { cqrs } from '../cqrs.js';

export const useResource = (id) => {
    const emptyState = () => ({ id: "", name: "", description: "", resourceGroup: "", virtualMachineExtentions: {}, virtualMachineScaleSetExtentions: null, environmentIds: [] });
    const [resource, setResource] = useState(emptyState);

    useEffect(() => get(id), [id]);

    const changeResource = (name, value) => {
        let properties = name.split('.');
        if (properties.length == 1) {
            setResource(prevState => { return {...prevState, [name]: value };});
        }
        else {
            let property = resource[properties[0]];
            let nestedObject = property;
            for (let i = 1; i < properties.length - 1; i++) {
                if (nestedObject[properties[i]] === undefined) {
                    nestedObject[properties[i]] = {};
                }
                nestedObject = nestedObject[properties[i]] || {};
            }
            nestedObject[properties[properties.length - 1]] = value;

            setResource(prevState => { return { ...prevState, [properties[0]]: property };});
        }
    };

    const get = (id) => {
        if (id === "") {
            setResource(emptyState);
        }
        else {
            cqrs('/api/resource/get', { id: id })
                .then((response) => response.json())
                .then((json) => setResource(json.resource));
        }
    }

    const upsert = (callback) => {
        var request = { 
            id: resource.id === "" ? null : resource.id,
            name: resource.name,
            description: resource.description,
            resourceGroup: resource.resourceGroup,
            environmentIds: resource.environmentIds,
            virtualMachineExtentions: resource.virtualMachineExtentions,
            virtualMachineScaleSetExtentions: resource.virtualMachineScaleSetExtentions
        };
        cqrs('/api/resource/upsert', request)
            .then((response) => response.json())
            .then((json) => {
                get(json.id);
                callback();
            });
    }

    return [resource, changeResource, upsert];
};

export const useResources = () => {
    const [resources, setResources] = useState([]);

    useEffect(() => get(), []);

    const get = () => {
        cqrs('/api/resource/list', {})
            .then((response) => response.json())
            .then((json) => setResources(json.resources))
            .catch(() => setResources([]));
    }

    const refetchResources = () => get();
    return { resources, refetchResources }
};
