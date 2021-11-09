import React from "react";
import { Input, Button, ButtonGroup } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import { useProvider } from './hooks';
import './styles.css';

export const ProviderForm = (props) => {
    const { id, removeSelectedProvider } = props;
    const [provider, changeProvider, upsert] = useProvider(id);

    const handleTypeChange = (az) => {
        changeProvider("azureProviderExtentions", az);
    };

    const handleChange = (event) => {
        const name = event.target.name;
        const value = event.target.value;
        changeProvider(name, value);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        upsert(removeSelectedProvider);
    };

    return (
        <>
            <div className="col right-align">
                <Button onClick={removeSelectedProvider} outline color="danger" type="submit">
                    <Locale id={"form-close"} locales={locales}>Close</Locale>
                </Button>
            </div>
            {!!!provider.id ? (
                <>
                    <div className="col">
                        <Locale id={"form-type"} locales={locales}>Cloud provider type</Locale>
                    </div>
                    <div className="col">
                        <ButtonGroup>
                            <Button color="info" onClick={() => handleTypeChange({ tenantId: "", clientId: "", clientSecret: "", subscriptionId: "" })}>
                                <Locale id={"form-type-az"} locales={locales}>Azure</Locale>
                            </Button>
                        </ButtonGroup>
                    </div>
                </>
            ) : null}
            <form onSubmit={handleSubmit}>
                <div className="col">
                    <Locale id={"form-id"} locales={locales}>Id</Locale>
                    <Input type="text" name="id" value={provider.id} disabled />
                </div>
                <div className="col">
                    <Locale id={"form-name"} locales={locales}>Name</Locale>
                    <Input type="text" name="name" value={provider.name} onChange={handleChange} />
                </div>
                {!!provider.azureProviderExtentions ? (
                    <>
                        <div className="col">
                            <Locale id={"form-az-tenant-id"} locales={locales}>Tenant id</Locale>
                            <Input type="text" name="azureProviderExtentions.tenantId" value={provider.azureProviderExtentions.tenantId} onChange={handleChange} />
                        </div>
                        <div className="col">
                            <Locale id={"form-az-subscription-id"} locales={locales}>Subscription id</Locale>
                            <Input type="text" name="azureProviderExtentions.subscriptionId" value={provider.azureProviderExtentions.subscriptionId} onChange={handleChange} />
                        </div>
                        <div className="col">
                            <Locale id={"form-az-client-id"} locales={locales}>Client id</Locale>
                            <Input type="text" name="azureProviderExtentions.clientId" value={provider.azureProviderExtentions.clientId} onChange={handleChange} />
                        </div>
                        <div className="col">
                            <Locale id={"form-az-client-secret"} locales={locales}>Client secret</Locale>
                            <Input type="password" name="azureProviderExtentions.clientSecret" value={provider.azureProviderExtentions.clientSecret} onChange={handleChange} />
                        </div>
                    </>
                ) : null}

                <div className="col save">
                    <Button color="info" type="submit">
                        <Locale id={"form-save"} locales={locales}>Save</Locale>
                    </Button>
                </div>
            </form>
        </>
    );
};
