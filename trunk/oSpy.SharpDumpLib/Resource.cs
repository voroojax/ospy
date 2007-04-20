﻿//
// Copyright (c) 2007 Ole André Vadla Ravnås <oleavr@gmail.com>
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;

namespace oSpy.SharpDumpLib
{
    public class Resource
    {
        protected UInt32 handle = 0;
        public UInt32 Handle
        {
            get { return handle; }
        }

        protected List<DataExchange> dataExchanges = new List<DataExchange>();
        public List<DataExchange> DataExchanges
        {
            get { return dataExchanges; }
        }

        private BulkStorage storage = null;

        public Resource(UInt32 handle)
        {
            this.handle = handle;
        }

        public virtual void Close()
        {
            foreach (DataExchange exchange in dataExchanges)
            {
                exchange.Close();
            }
            dataExchanges.Clear();

            if (storage != null)
            {
                storage.Close();
                storage = null;
            }
        }

        public virtual DataExchange AppendData(byte[] data, DataDirection direction)
        {
            DataExchange exchange = null;

            if (DataIsContinuous())
            {
                if (dataExchanges.Count == 0)
                {
                    dataExchanges.Add(new DataExchange(this));
                }
                exchange = dataExchanges[0];
                exchange.Append(data, direction);
            }
            else
            {
                if (storage == null)
                {
                    storage = new BulkStorage();
                }

                exchange = new DataExchange(this, storage.AppendData(data), direction);
                dataExchanges.Add(exchange);
            }

            return exchange;
        }

        protected virtual bool DataIsContinuous()
        {
            return false;
        }
    }

    public enum ResourceType
    {
        Unknown,
        Socket,
        CryptoContext,
    }
}