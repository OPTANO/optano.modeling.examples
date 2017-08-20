using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NDP;
using System.Collections.Generic;
using OPTANO.Modeling.Optimization.Configuration;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Solver.Gurobi702;
using OPTANO.Modeling.Common;
using System.Linq;
/// <summary>
/// This is a Demo Test Project for NDP
/// </summary>
namespace MIP_NDP_Tests
{
    /// <summary>
    /// A demo of test cases for NDP
    /// 
    /// ATTENTION: CHOOSE THE PLATTFORM OF YOUR SOLVER BEFORE RUNNING TESTS: x64 // x86
    /// </summary>
    [TestClass]
    public class NDP_Test1
    {
        /// <summary>
        /// Modeling Scope for all Tests
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// Re-used network design model for all tests
        /// </summary>
        private NetworkDesignModel designModel;

        /// <summary>
        /// List of nodes
        /// </summary>
        private List<INode> nodes;

        /// <summary>
        /// List of edges
        /// </summary>
        private List<IEdge> edges;

        /// <summary>
        /// Test initializer, generating the network and setting all data required by tests 
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            // create example Nodes
            INode pb = new Node("Paderborn", 50);
            INode ny = new Node("New York", -100);
            INode b = new Node("Beijing", 50);
            INode sp = new Node("São Paulo", 50);
            INode sf = new Node("San Francisco", -50);
            // assign these nodes to a list of INodes
            nodes = new List<INode> { pb, ny, b, sp, sf };

            // create example Edges
            IEdge one = new Edge(ny, pb, null, 3, 6100);
            IEdge two = new Edge(b, ny, null, 2, 11000);
            IEdge three = new Edge(sp, b, 75, 1, 17600);
            IEdge four = new Edge(sf, sp, null, 4, 10500);
            IEdge five = new Edge(sp, pb, null, 5, 9900);
            IEdge six = new Edge(pb, b, 50, 1, 7600);
            // assign these edges to a list of IEdges
            edges = new List<IEdge> { one, two, three, four, five, six };

            // use default settings
            var config = new Configuration();
            config.EnableFullNames = true;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the model scope
                designModel = new NetworkDesignModel(nodes, edges);
                // Get a solver instance, change your solver
                var solver = new GurobiSolver();

                // solve the model
                // if this fails: check if this project references the solver. Add one and update the using, if required.
                var solution = solver.Solve(designModel.Model);

                // import the results back into the model 
                designModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));
                scope.ModelBehavior = OPTANO.Modeling.Optimization.Configuration.ModelBehavior.Manual;
            
            }

        }

        /// <summary>
        /// Test flow constraint
        /// </summary>
        [TestMethod]
        public void TestNDPFlowBalance()
        {
            foreach (var node in nodes)
            {
                // Test is flow balance is correct in the tolerance of epsilon
                Assert.IsTrue(edges.Where(e => e.FromNode == node).Select(e => designModel.x[e].Value).Sum() + node.Demand // all leaving flow and demand
                    - (edges.Where(e => e.ToNode == node).Select(e => designModel.x[e].Value).Sum()) // minus all incoming flow
                    <= configuration.Epsilon); // must be zero (is a match)
            }
        }

        /// <summary>
        /// Tests if a minimal flow is given
        /// </summary>
        [TestMethod]
        public void TestNDPMinimalFlow()
        {
           if (nodes.Any(n => n.Demand > 0) && edges.Select(e => designModel.x[e].Value).Sum() <= configuration.Epsilon)
            {
                Assert.Fail();
            }
        }
    }
}
