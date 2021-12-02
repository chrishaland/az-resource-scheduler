import { useState, useEffect } from "react";
import { useEnvironmentsStore } from "./store";
import { cqrs } from '../cqrs.js';
import { useReactOidc } from "@axa-fr/react-oidc-context";

export const useEnvironment = (id) => {
    const emptyState = () => ({ id: "", name: "", description: "", scheduledStartup: "0 7 * * *", scheduledUptime: 0, timeZoneId: "W. Europe Standard Time" });
    const [environment, setEnvironment] = useState(emptyState);
    const { oidcUser } = useReactOidc();

    useEffect(() => get(id), [id]);

    const changeEnvironment = (name, value) => setEnvironment({ ...environment, [name]: value });

    const get = (id) => {
        if (id === "") {
            setEnvironment(emptyState);
        }
        else {
            cqrs('/api/environment/get', { id: id }, oidcUser.id_token)
                .then((response) => response.json())
                .then((json) => setEnvironment(json.environment));
        }

    }

    const upsert = (callback) => {
        var request = { 
            id: environment.id === "" ? null : environment.id,
            name: environment.name,
            description: environment.description,
            scheduledStartup: environment.scheduledStartup,
            scheduledUptime: environment.scheduledUptime,
            timeZoneId: environment.timeZoneId
        };
        cqrs('/api/environment/upsert', request, oidcUser.id_token)
            .then((response) => response.json())
            .then((json) => {
                get(json.id);
                callback();
            });
    }

    return [environment, changeEnvironment, upsert];
};

export const useEnvironments = () => {
    const { state, dispatch } = useEnvironmentsStore();
    const { oidcUser } = useReactOidc();

    const refetchEnvironments = () => {
        cqrs('/api/environment/list', {}, oidcUser.id_token)
            .then((response) => response.json())
            .then((json) => dispatch({ type: "setEnvironments", environments: json.environments }));
    }
    return { environments: state.environments, refetchEnvironments }
};
