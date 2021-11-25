import React from 'react';
import { Table, Button } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import './styles.css';

export const ProviderList = (props) => {
    const { providers, onSelectProvider } = props;

    return (
        <Table>
            <thead>
                <tr>
                    <th>#</th>
                    <th>
                        <Locale id={"list-header-name"} locales={locales}>Name</Locale>
                    </th>
                    <th className="right-align">
                        <Button color="info" onClick={() => onSelectProvider("")}>
                            <Locale id={"add"} locales={locales}>Add</Locale>
                        </Button>
                    </th>
                </tr>
            </thead>
            <tbody>
                {providers.sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0)).map((provider, index) => (
                    <tr key={index + 1}>
                        <th scope="row">{index + 1}</th>
                        <td>{provider.name}</td>
                        <td className="right-align">
                            <Button color="info" onClick={() => onSelectProvider(provider.id)}>
                                <Locale id={"list-header-edit"} locales={locales}>Edit</Locale>
                            </Button>
                        </td>
                    </tr>
                ))}
            </tbody>
        </Table>
    );
};
