import { useState, useEffect } from "react";
import { useEnvironmentsStore } from "./store";
import { cqrs } from '../cqrs.js';

export const useEnvironment = (id) => {
    const [environment, setEnvironment] = useState({ id: "", name: "" });

    useEffect(() => get(id), [id]);

    const changeEnvironment = (name, value) => setEnvironment({ ...environment, [name]: value });

    const get = (id) => {
        if (id === "") {
            setEnvironment({ id: "", name: "" });
        }
        else {
            cqrs('/api/environment/get', { id: id })
                .then((response) => response.json())
                .then((json) => setEnvironment(json.environment));
        }

    }

    const upsert = (callback) => {
        cqrs('/api/environment/upsert', { id: environment.id === "" ? null : environment.id, name: environment.name })
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

    const refetchEnvironments = () => {
        cqrs('/api/environment/list', {})
            .then((response) => response.json())
            .then((json) => dispatch({ type: "setEnvironments", environments: json.environments }));
    }
    return { environments: state.environments, refetchEnvironments }
};
