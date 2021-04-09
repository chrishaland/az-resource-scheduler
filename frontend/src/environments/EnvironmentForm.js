import React from "react";
import { Input, Button } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { useEnvironment } from './hooks';
import './styles.css';

export const EnvironmentForm = (props) => {
    const { id, onSelectEnvironment, removeSelectedEnvironment } = props;

    const [environment, changeEnvironment, upsert] = useEnvironment(id);

    const handleChange = (event) => {
        const name = event.target.name;
        const value = event.target.value;
        changeEnvironment(name, value);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        upsert(removeSelectedEnvironment);
    };

    return (
        <>
            <div className="col right-align">
                <Button onClick={removeSelectedEnvironment} outline color="danger" type="submit">
                    <Locale id={"form-close"} locales={locales}>Close</Locale>
                </Button>
            </div>
            <form onSubmit={handleSubmit}>
                <div className="col">
                    <Locale id={"form-id"} locales={locales}>Id</Locale>
                    <Input type="text" name="id" value={environment.id} disabled />
                </div>
                <div className="col">
                    <Locale id={"form-name"} locales={locales}>Name</Locale>
                    <Input type="text" name="name" value={environment.name} onChange={handleChange} />
                </div>
                <div className="col">
                    <Locale id={"form-description"} locales={locales}>Description</Locale>
                    <Input type="text" name="description" value={environment.description} onChange={handleChange} />
                </div>
                <div className="col">
                    <Locale id={"form-scheduledStartup"} locales={locales}>Scheduled startup (cron expression)</Locale>
                    <Input type="text" name="scheduledStartup" value={environment.scheduledStartup} onChange={handleChange} />
                </div>
                <div className="col">
                    <Locale id={"form-scheduledUptime"} locales={locales}>Scheduled uptime (hours)</Locale>
                    <Input type="text" name="scheduledUptime" value={environment.scheduledUptime} onChange={handleChange} />
                </div>
                <div className="col">
                    <Button color="info" type="submit">
                        <Locale id={"form-save"} locales={locales}>Save</Locale>
                    </Button>
                </div>
            </form>
        </>
    );
};
