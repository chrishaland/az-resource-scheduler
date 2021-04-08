import React from 'react';
import { Route } from 'react-router';
import { Layout } from './app/Layout';
import { Scheduler } from './scheduler/Scheduler';
import { Resources } from './resources/Resources';
import { Environments } from './environments/Environments';
import { LanguageProvider } from './translations/store';
import { AccountProvider } from './accounts/store';
import { EnvironmentsProvider } from './environments/store';

export const App = () => {
    return (
        <LanguageProvider>
            <AccountProvider>
                <EnvironmentsProvider>
                    <Layout>
                        <Route exact path='/' component={Scheduler} />
                            <Route path='/resources' component={Resources} />
                        <Route path='/environments' component={Environments} />
                    </Layout>
                </EnvironmentsProvider>
            </AccountProvider>
        </LanguageProvider>
    );
}

export default App;
