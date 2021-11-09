import { useState, useEffect } from "react";
import { cqrs } from '../cqrs.js';
import { useReactOidc } from "@axa-fr/react-oidc-context";

export const useProvider = (id) => {
    const emptyState = () => ({ id: "", name: "", azureProviderExtentions: { tenantId: "", clientId: "", clientSecret: "", subscriptionId: "" } });
    const [provider, setProvider] = useState(emptyState);
    const { oidcUser } = useReactOidc();

    useEffect(() => get(id), [id]);

    const changeProvider = (name, value) => {
        let properties = name.split('.');
        if (properties.length == 1) {
            setProvider(prevState => { return {...prevState, [name]: value };});
        }
        else {
            let property = provider[properties[0]];
            let nestedObject = property;
            for (let i = 1; i < properties.length - 1; i++) {
                if (nestedObject[properties[i]] === undefined) {
                    nestedObject[properties[i]] = {};
                }
                nestedObject = nestedObject[properties[i]] || {};
            }
            nestedObject[properties[properties.length - 1]] = value;

            setProvider(prevState => { return { ...prevState, [properties[0]]: property };});
        }
    };

    const get = (id) => {
        if (id === "") {
            setProvider(emptyState);
        }
        else {
            cqrs('/api/provider/get', { id: id }, oidcUser.id_token)
                .then((response) => response.json())
                .then((json) => setProvider(json.provider));
        }
    }

    const upsert = (callback) => {
        var request = { 
            id: provider.id === "" ? null : provider.id,
            name: provider.name,
            azureProviderExtentions: provider.azureProviderExtentions
        };
        cqrs('/api/provider/upsert', request, oidcUser.id_token)
            .then((response) => response.json())
            .then((json) => {
                get(json.id);
                callback();
            });
    }

    return [provider, changeProvider, upsert];
};

export const useProviders = () => {
    const [providers, setProviders] = useState([]);
    const { oidcUser } = useReactOidc();

    useEffect(() => get(), []);

    const get = () => {
        cqrs('/api/provider/list', {}, oidcUser.id_token)
            .then((response) => response.json())
            .then((json) => setProviders(json.providers))
            .catch(() => setProviders([]));
    }

    const refetchProviders = () => get();
    return { providers, refetchProviders }
};
